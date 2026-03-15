import os
import psycopg2
from pgvector.psycopg2 import register_vector
from dotenv import load_dotenv
# noinspection PyUnresolvedReferences
from langchain_community.document_loaders import PyPDFLoader
from langchain_text_splitters import RecursiveCharacterTextSplitter
from litellm import embedding
from fastapi import FastAPI, HTTPException, Security
from pydantic import BaseModel, Field
from typing import List, Optional
import time


load_dotenv()
# ========================================
# 1. CONFIGURACIÓN DEL FASTAPI
# ========================================
app = FastAPI(
    title = "AinaLabs AI Engine",
    description = "Microservicio de IA Seguro para Blazor/.NET",
    version = "1.0.0"
)

# ========================================
# 2. MODELOS DE ENTRADA (Request DTOs)
# ========================================
class FileMetadata(BaseModel):
    file_id: str
    filename: str
    storage_path: str

class AnalysisConfig(BaseModel):
    language: str = Field(default="es", description="Idioma del análisis")
    focus: str = Field(default="general", description="Enfoque: legal_risk, financial, general")
    generate_summary: bool = True

class AnalyzePdfRequest(BaseModel):
    tenant_id: str = Field(..., description="ID del cliente para aislamiento RLS")
    user_id: str
    file_metadata: FileMetadata
    analysis_config: AnalysisConfig

# ==========================================
# 3. MODELOS DE SALIDA (Response DTOs)
# ==========================================
class KeyFinding(BaseModel):
    topic: str
    risk_level: str
    description: str

class AnalysisResult(BaseModel):
    summary: str
    key_findings: List[KeyFinding]
    entities: List[str]
    vector_index_status: str

class UsageStats(BaseModel):
    tokens_consumed: int
    model_used: str

class AnalyzePdfResponse(BaseModel):
    status: str
    analysis_result: AnalysisResult
    usage_stats: UsageStats

# ==========================================
# 4. ENDPOINTS (Controladores)
# ==========================================
@app.post("/v1/formulas/analyze-pdf", response_model=AnalyzePdfResponse)
async def analyze_pdf(request: AnalyzePdfRequest):
    print(f"🧠 Iniciando análisis real para Tenant: {request.tenant_id}")

    try:
        # 1. LEER EL PDF
        # Para esta prueba, asegúrate de poner un archivo "prueba.pdf" en la carpeta de Python
        # En el futuro, usaremos request.file_metadata.storage_path
        pdf_path = "prueba.pdf"

        if not os.path.exists(pdf_path):
            raise Exception(f"No se encuentra el archivo {pdf_path} para procesar.")

        loader = PyPDFLoader(pdf_path)
        document_pages = loader.load()
        texto_completo = " ".join([page.page_content for page in document_pages])
        print(f"📄 PDF leído: {len(document_pages)} páginas.")

        # 2. TROCEAR EL DOCUMENTO (Chunking)
        # La IA no puede leer 100 páginas de golpe. Lo partimos en trozos lógicos.
        text_splitter = RecursiveCharacterTextSplitter(
            chunk_size=1000, # 1000 caracteres por trozo
            chunk_overlap=200 # Solapamos 200 para no cortar frases a la mitad
        )
        chunks = text_splitter.split_text(texto_completo)
        print(f"✂️ Documento dividido en {len(chunks)} fragmentos.")

        # 3. GENERAR VECTORES (Agnóstico)
        # Aquí es donde en el futuro leeremos de la base de datos qué modelo prefiere la Empresa A
        # Por ahora, forzamos Gemini para probar tu API Key
        model_name = "gemini/text-embedding-004"

        print(f"🤖 Generando vectores con el modelo: {model_name}...")
        response = embedding(
            model=model_name,
            input=chunks
        )
        embeddings = [item['embedding'] for item in response.data]

        # Detectamos automáticamente el tamaño del vector devuelto
        vector_size = len(embeddings[0])
        print(f"📏 Tamaño del vector detectado: {vector_size} dimensiones")

        # 4. GUARDAR EN POSTGRESQL (Enrutamiento dinámico)
        print("💾 Guardando en base de datos segura...")
        conn = psycopg2.connect(
            dbname="ainalabs_db", user="ainalabs_admin", password="super_password_seguro_123", host="127.0.0.1", port="5432"
        )
        register_vector(conn)
        cur = conn.cursor()

        for chunk_text, chunk_embedding in zip(chunks, embeddings):
            # MAGIA: Dependiendo del tamaño, insertamos en una columna o en otra
            if vector_size == 768:
                cur.execute(
                    """
                    INSERT INTO document_chunks (tenant_id, document_id, content, model_used, embedding_768)
                    VALUES (%s, %s, %s, %s, %s)
                    """,
                    (request.tenant_id, request.file_metadata.file_id, chunk_text, model_name, chunk_embedding)
                )
            elif vector_size == 1536:
                cur.execute(
                    """
                    INSERT INTO document_chunks (tenant_id, document_id, content, model_used, embedding_1536)
                    VALUES (%s, %s, %s, %s, %s)
                    """,
                    (request.tenant_id, request.file_metadata.file_id, chunk_text, model_name, chunk_embedding)
                )
            else:
                raise Exception(f"Tamaño de vector no soportado: {vector_size}")

        conn.commit()
        cur.close()
        conn.close()
        print("✅ Proceso completado con éxito.")

        # 5. DEVOLVER RESPUESTA A .NET
        mock_finding = KeyFinding(
            topic="Análisis completado",
            risk_level="low",
            description=f"Se han procesado {len(chunks)} fragmentos y guardado en la base de datos vectorial."
        )

        return AnalyzePdfResponse(
            status="success",
            analysis_result=AnalysisResult(
                summary=f"Documento procesado correctamente.",
                key_findings=[mock_finding],
                entities=[],
                vector_index_status="ready"
            ),
            usage_stats=UsageStats(tokens_consumed=response.usage.total_tokens, model_used="text-embedding-3-small")
        )

    except Exception as e:
        print(f"❌ Error en Python: {str(e)}")
        raise HTTPException(status_code=500, detail=str(e))
    
# Endpoint de salud para monitorización
@app.get("/api/v1/health")
async def health_check():
    return {"status": "healthy", "service": "AinaLabs AI Engine"}
        