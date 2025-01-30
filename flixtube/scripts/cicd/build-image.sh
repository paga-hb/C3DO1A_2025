#
# Builds a Docker image.
#
# Environment variables:
#
#   AZURE_CONTAINER_REGISTRY_HOSTNAME - The hostname of your container registry.
#   IMAGE_NAME - The name of the image to build.
#   IMAGE_VERSION - The version number to tag the images with.
#   MICROSERVICE_DIRECTORY - The directory from which to build the image.
#
# Usage:
#
#       ./scripts/cicd/build-image.sh
#

set -u # or set -o nounset
: "$AZURE_CONTAINER_REGISTRY_HOSTNAME"
: "$IMAGE_NAME"
: "$IMAGE_VERSION"
: "$MICROSERVICE_DIRECTORY"

docker build -t $AZURE_CONTAINER_REGISTRY_HOSTNAME/$IMAGE_NAME:$IMAGE_VERSION --file ./$MICROSERVICE_DIRECTORY/Dockerfile-prod ./$MICROSERVICE_DIRECTORY
