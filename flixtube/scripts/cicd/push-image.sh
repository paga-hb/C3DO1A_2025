#
# Publishes a Docker image.
#
# Environment variables:
#
#   AZURE_CONTAINER_REGISTRY_HOSTNAME - The hostname of your container registry.
#   AZURE_CONTAINER_REGISTRY_USERNAME - User name for your container registry.
#   AZURE_CONTAINER_REGISTRY_PASSWORD - Password for your container registry.
#   IMAGE_NAME - The name of the image.
#   IMAGE_VERSION - The image's version number (tag).
#
# Usage:
#
#       ./scripts/cicd/push-image.sh
#

set -u # or set -o nounset
: "$AZURE_CONTAINER_REGISTRY_HOSTNAME"
: "$AZURE_CONTAINER_REGISTRY_USERNAME"
: "$AZURE_CONTAINER_REGISTRY_PASSWORD"
: "$IMAGE_NAME"
: "$IMAGE_VERSION"

echo $AZURE_CONTAINER_REGISTRY_PASSWORD | docker login $AZURE_CONTAINER_REGISTRY_HOSTNAME --username $AZURE_CONTAINER_REGISTRY_USERNAME --password-stdin
docker push $AZURE_CONTAINER_REGISTRY_HOSTNAME/$IMAGE_NAME:$IMAGE_VERSION
