#!/bin/sh

# IMPORTANT: If you modify this file, ensure that you use LF line endings
#            amd run the following command to ensure that the file is executable:
#            git update-index --chmod=+x ./scripts/development/localstack/init.sh

# enable debug
# set -x

create_queue() {
    local QUEUE_NAME_TO_CREATE=$1
    awslocal sqs create-queue --queue-name ${QUEUE_NAME_TO_CREATE} --attributes VisibilityTimeout=30
}

create_bucket() {
    local BUCKET_NAME=$1
    awslocal s3api create-bucket --bucket ${BUCKET_NAME}
    awslocal s3api put-bucket-cors --bucket ${BUCKET_NAME} --cors-configuration '{"CORSRules": [ {"ID": "02","AllowedHeaders": ["*"],"AllowedMethods": ["GET", "PUT", "POST", "DELETE", "HEAD"],"AllowedOrigins": ["*"],"ExposeHeaders": [],"MaxAgeSeconds": 3000 }]}'
    awslocal s3api get-bucket-cors --bucket ${BUCKET_NAME} --output json
}

upload_bucket_folder() {
    local PATH_SOURCE=$1
    local PATH_DEST=$2

    awslocal s3 cp ${PATH_SOURCE} ${PATH_DEST} --recursive
}

verify_email() {
    local EMAIL_TO_VERIFY=$1
    awslocal ses verify-email-identity --email-address ${EMAIL_TO_VERIFY}
}

# echo "########### Install cdk ###########"
# npm install -g aws-cdk-local aws-cdk
# cdklocal --version

# Required for Shared integration test
echo "################ Start creating Shared SQS..."
create_queue "simple-queue-test"
create_queue "typed-queue-test"
echo "################ Start creating Shared S3..."
create_bucket "mybucket"
echo "################ Start creating Shared SES..."
verify_email "no-reply@m47labs.com"

# Required for Hal2b integration test
echo "################ Start creating Hal2b SQS..."
create_queue "hal2b-processbook"
create_queue "hal2b-stripe"
echo "################ Start creating Hal2b S3..."
create_bucket "hal2b-salander-datasets"
create_bucket "hal2b-salander-stripe"
echo "################ Upload required Hal2b S3 files..."
upload_bucket_folder "/files/explanations" "s3://hal2b-salander-datasets/explanations/"
echo "################ Start creating Hal2b SES..."
verify_email "no-reply@hal2b.com"

# List resources created
echo "################ Listing SQS ###########"
awslocal sqs list-queues
echo "################ Listing S3 ###########"
awslocal s3api list-buckets
echo "################ Listing SES ###########"
awslocal ses list-identities

exit 0