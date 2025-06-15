@echo off
cd /d "%~dp0"
cd ..\API_TFG
uvicorn main:app --host 127.0.0.1 --port 8000 --reload
pause

