@echo off
docker build -t copypastas-note . && heroku container:push -a copypastas-notebook web && heroku container:release -a copypastas-notebook web