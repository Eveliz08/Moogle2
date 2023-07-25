#!/bin/bash

# Función para regresar al directorio Script
regresar_a_script() {
  cd "$(dirname "$0")"
}

# Función para compilar Moogle!
compilar_proyecto() {
  cd ..
  dotnet watch run --project MoogleServer
  regresar_a_script
}

# Función para compilar y generar el PDF del informe
compilar_informe() {
  cd ..
  cd Informe
  latexmk -pdf informe.tex
  regresar_a_script
}

# Función para compilar y generar el PDF de la presentación
compilar_presentacion() {
  cd ..
  cd Presentación
  latexmk -pdf presentacion.tex
  regresar_a_script
}

# Función para mostrar el informe
mostrar_informe() {
  if [ ! -f ../Informe/informe.pdf ]; then
    compilar_informe
  fi
  if [ $# -eq 0 ]; then
    start ../Informe/informe.pdf
  else
    "$@" ../Informe/informe.pdf
  fi
}

# Función para mostrar la presentación
mostrar_presentacion() {
  if [ ! -f ../Presentación/presentacion.pdf ]; then
    compilar_presentacion
  fi
  if [ $# -eq 0 ]; then
    start ../Presentación/presentacion.pdf
  else
    "$@" ../Presentación/presentacion.pdf
  fi
}

# Función para limpiar los archivos auxiliares
limpiar_archivos_auxiliares() {
  cd ../Informe
  latexmk -c
  cd ../Presentación
  latexmk -c
  cd ../..
  regresar_a_script
}

# Menú principal
while true; do
  echo "Selecciona una opción:"
  echo "1. Ejecutar el proyecto"
  echo "2. Compilar y generar el informe"
  echo "3. Compilar y generar la presentación"
  echo "4. Mostrar el informe"
  echo "5. Mostrar la presentación"
  echo "6. Limpiar archivos auxiliares"
  echo "7. Salir"
  read opcion
  case $opcion in
    1)
      compilar_proyecto
      ;;
    2)
      compilar_informe
      ;;
    3)
      compilar_presentacion
      ;;
    4)
      mostrar_informe "$@"
      ;;
    5)
      mostrar_presentacion "$@"
      ;;
    6)
      limpiar_archivos_auxiliares
      ;;
    7)
      exit
      ;;
    *)
      echo "Opción inválida"
      ;;
  esac
done
