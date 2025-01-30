#!/bin/bash

# Wait for Minio to become available
while [ ! $(mc alias set minio http://minio:9000 minio_user minio_password) ]
# while [ ! $(curl -I http://minio:9000/minio/health/live) ]
do
  echo 'Waiting for minio to start up ...'
  sleep 0.1
done
sleep 5

# Create public bucket with object
/usr/bin/mc alias set minio http://minio:9000 minio_user minio_password
/usr/bin/mc mb minio/videos
/usr/bin/mc anonymous set public minio/videos
/usr/bin/mc put /tmp/videos/5d9e690ad76fe06a3d7ae416 minio/videos

# Create user with readwrite and deleteobject access
/usr/bin/mc admin user add minio Y42xk0CJlYV2fnShCtBP kCA01XftotivPbU9TRJ5Nrr5VMjFZNEhQhtvfWyY
/usr/bin/mc admin policy attach minio readwrite --user Y42xk0CJlYV2fnShCtBP

# Exit shell (so that container terminates)
exit 0