from passlib.context import CryptContext
from sqlalchemy.orm import Session
from database import SessionLocal
from models import Usuario
from schemas import UsuarioIn
from fastapi import HTTPException

pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")

def hash_password(password: str) -> str:
    return pwd_context.hash(password)

def verify_password(plain_password: str, hashed_password: str) -> bool:
    return pwd_context.verify(plain_password, hashed_password)

def registrar_usuario(usuario: UsuarioIn):
    db: Session = SessionLocal()
    existente = db.query(Usuario).filter_by(nombre=usuario.nombre).first()
    if existente:
        raise HTTPException(status_code=400, detail="Usuario ya existe")

    nuevo = Usuario(
        nombre=usuario.nombre,
        hashed_password=hash_password(usuario.password),
        puntuacion=usuario.puntuacion,
        tiempo=usuario.tiempo
    )
    db.add(nuevo)
    db.commit()
    db.refresh(nuevo)
    return {"mensaje": "Registrado", "usuario": nuevo.nombre}


def autenticar_usuario(nombre: str, password: str):
    db: Session = SessionLocal()
    user = db.query(Usuario).filter_by(nombre=nombre).first()
    if not user or not verify_password(password, user.hashed_password):
        return None
    return user


def get_puntuacion(nombre: str):
    db: Session = SessionLocal()
    usuario = db.query(Usuario).filter_by(nombre=nombre).first()
    if not usuario:
        raise HTTPException(status_code=404, detail="No encontrado")
    return {
        "nombre": usuario.nombre,
        "puntuacion": usuario.puntuacion,
        "tiempo": usuario.tiempo   
    }


def actualizar_puntuacion(nombre: str, nueva_puntuacion: int, nuevo_tiempo: float):
    db: Session = SessionLocal()
    usuario = db.query(Usuario).filter_by(nombre=nombre).first()

    if not usuario:
        raise HTTPException(status_code=404, detail="No encontrado")

    if nueva_puntuacion >= usuario.puntuacion:
        if nuevo_tiempo <= usuario.tiempo or usuario.tiempo == 0:
            usuario.puntuacion = nueva_puntuacion
            usuario.tiempo = nuevo_tiempo
            db.commit()
            return {"mensaje": "Puntuación y tiempo actualizados"}
        else:
            return {"mensaje": "El tiempo no mejora, no se actualiza"}
    else:
        return {"mensaje": "La puntuación es menor, no se actualiza"}



