from fastapi import FastAPI, Depends, HTTPException, status
from fastapi.security import OAuth2PasswordBearer, OAuth2PasswordRequestForm
from auth import create_access_token, decode_token
from crud import registrar_usuario, autenticar_usuario, get_puntuacion, actualizar_puntuacion
from schemas import UsuarioIn, UsuarioLogin, ActualizarRequest, Token
from database import engine, get_db
from models import Base
from sqlalchemy.orm import Session

Base.metadata.create_all(bind=engine)
app = FastAPI()

oauth2_scheme = OAuth2PasswordBearer(tokenUrl="login")

@app.post("/registrar/")
def registrar(usuario: UsuarioIn):
    return registrar_usuario(usuario)

@app.post("/login", response_model=Token)
def login(form_data: OAuth2PasswordRequestForm = Depends()):
    user = autenticar_usuario(form_data.username, form_data.password)
    if not user:
        raise HTTPException(status_code=401, detail="Credenciales inválidas")

    token = create_access_token({"sub": user.nombre})
    return {"access_token": token, "token_type": "bearer"}

# Middleware para autenticación
def get_current_user(token: str = Depends(oauth2_scheme)):
    nombre = decode_token(token)
    if not nombre:
        raise HTTPException(status_code=401, detail="Token inválido")
    return nombre

@app.get("/usuario/{nombre}")
def puntuacion(nombre: str):
    return get_puntuacion(nombre)

@app.post("/actualizar/")
def actualizar(req: ActualizarRequest, nombre: str = Depends(get_current_user)):
    return actualizar_puntuacion(nombre, req.puntuacion, req.tiempo)


