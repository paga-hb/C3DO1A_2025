#
# Builds and deploys all microservices to a local Kubernetes instance.
#
# Usage:
#
#   ./scripts/local-k8-cluster/deploy.sh
#

#
# Build Docker images.
#
docker build -t video-storage:1 --file ../../Flixtube.AzureStorage/Dockerfile-prod ../../Flixtube.AzureStorage
#docker build -t video-storage:1 --file ../../Flixtube.MinioStorage/Dockerfile-prod ../../Flixtube.MinioStorage
docker build -t video-upload:1 --file ../../Flixtube.VideoUpload/Dockerfile-prod ../../Flixtube.VideoUpload
docker build -t video-streaming:1 --file ../../Flixtube.VideoStreaming/Dockerfile-prod ../../Flixtube.VideoStreaming
docker build -t metadata:1 --file ../../Flixtube.Metadata/Dockerfile-prod ../../Flixtube.Metadata
docker build -t history:1 --file ../../Flixtube.History/Dockerfile-prod ../../Flixtube.History
docker build -t gateway:1 --file ../../Flixtube.Gateway/Dockerfile-prod ../../Flixtube.Gateway
docker build -t web:1 --file ../../Flixtube.Web/Dockerfile-prod ../../Flixtube.Web

# 
# Deploy containers to Kubernetes.
#
# Don't forget to change kubectl to your local Kubernetes instance, like this:
#
#   kubectl config use-context docker-desktop
#
kubectl apply -f rabbit.yaml
kubectl apply -f sqlserver.yaml
kubectl apply -f minio.yaml
kubectl apply -f video-storage.yaml
kubectl apply -f video-upload.yaml
kubectl apply -f video-streaming.yaml
kubectl apply -f metadata.yaml
kubectl apply -f history.yaml
kubectl apply -f gateway.yaml
kubectl apply -f web.yaml