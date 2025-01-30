#
# Tests a microservice.
#
# Environment variables:
#
#   NAME - The name of the microservice to test.
#   CONTAINER_NAME - The container's name.
#
# Usage:
#
#       ./scripts/cicd/test.sh
#

set -u # or set -o nounset
: "$NAME"
: "$CONTAINER_NAME"

envsubst < ./compose/docker-compose-$NAME-dev.yml | docker compose -f - --project-directory . up -d
until docker exec $CONTAINER_NAME curl -f http://localhost:80/health; do
  echo "Waiting for the service to be ready..."
  sleep 5
done
docker exec $CONTAINER_NAME dotnet test
TEST_EXIT_CODE=$?

# Copy screenshots from Web container to host
if [[ ${CONTAINER_NAME,,} == "web" ]]; then
  docker cp $CONTAINER_NAME:/screenshots ./screenshots
fi

envsubst < ./compose/docker-compose-$NAME-dev.yml | docker compose -f - --project-directory . down
exit $TEST_EXIT_CODE