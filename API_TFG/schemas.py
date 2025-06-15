from pydantic import BaseModel

class UsuarioIn(BaseModel):
    nombre: str
    password: str
    puntuacion: int = 0
    tiempo: float = 0.0

class UsuarioLogin(BaseModel):
    nombre: str
    password: str

class Token(BaseModel):
    access_token: str
    token_type: str


class UsuarioOut(BaseModel):
    nombre: str
    puntuacion: int
    tiempo: float

class ActualizarRequest(BaseModel):   
    puntuacion: int
    tiempo: float


