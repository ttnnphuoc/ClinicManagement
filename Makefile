.PHONY: help install-backend install-frontend install migrate migrate-create db-update run-backend run-frontend dev clean build

help:
	@echo "Available commands:"
	@echo "  make install          - Install both backend and frontend dependencies"
	@echo "  make install-backend  - Install backend dependencies"
	@echo "  make install-frontend - Install frontend dependencies"
	@echo "  make migrate-create   - Create a new migration (use NAME=MigrationName)"
	@echo "  make migrate          - Apply pending migrations"
	@echo "  make db-update        - Update database with migrations"
	@echo "  make run-backend      - Run backend API"
	@echo "  make run-frontend     - Run frontend dev server"
	@echo "  make dev              - Run both frontend and backend concurrently"
	@echo "  make build            - Build both projects"
	@echo "  make clean            - Clean build artifacts"

install: install-backend install-frontend

install-backend:
	@echo "Installing backend dependencies..."
	dotnet restore

install-frontend:
	@echo "Installing frontend dependencies..."
	cd client && npm install

migrate-create:
	@echo "Creating migration: $(NAME)"
	cd src/ClinicManagement.Infrastructure && \
	dotnet ef migrations add $(NAME) --startup-project ../ClinicManagement.API

migrate:
	@echo "Applying migrations..."
	cd src/ClinicManagement.Infrastructure && \
	dotnet ef database update --startup-project ../ClinicManagement.API

db-update: migrate

run-backend:
	@echo "Starting backend API..."
	cd src/ClinicManagement.API && dotnet run

run-frontend:
	@echo "Starting frontend dev server..."
	cd client && npm run dev

dev:
	@echo "Starting both frontend and backend..."
	@make -j 2 run-backend run-frontend

build:
	@echo "Building backend..."
	dotnet build
	@echo "Building frontend..."
	cd client && npm run build

clean:
	@echo "Cleaning build artifacts..."
	dotnet clean
	rm -rf client/dist
	rm -rf client/node_modules/.vite