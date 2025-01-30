# 
# Remove containers from Kubernetes.
#
# Environment variables:
#
#   MICROSERVICE_NAME - The name of the microservice to delete.
#
# Usage:
#
#   ./scripts/cicd/delete.sh
#
set -u # or set -o nounset
: "$MICROSERVICE_NAME"

envsubst < ./scripts/cd/${MICROSERVICE_NAME}.yaml | kubectl delete -f -