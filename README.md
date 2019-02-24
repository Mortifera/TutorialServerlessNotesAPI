# TutorialServerlessNotesAPI

Read the following blog posts for understanding of how this was made and why:
* [Part 1](https://blog.gardnersoftware.co.uk/posts/2019-02-10/serverless-webapi-part-1/)
* [Part 2](https://blog.gardnersoftware.co.uk/posts/2019-02-12/serverless-webapi-part-2/)
* [Part 3](https://blog.gardnersoftware.co.uk/posts/2019-02-25/serverless-webapi-part-3/)

## Overview

Creates a DynamoDB, Lambda and API Gateway.

The DynamoDB stores notes. A "note" is a NoteId *(string, database primary key)*, Title *(string)*, Content *(string)*, and ModifiedTime *(epoch number)*. 

The API Gateway sets up a REST API at two paths '/v1/notes' and '/v1/notes/{note_id}' where {note_id} can be any note id string such as ```48154dc3-5f3d-42ff-b17f-0f679a345914```.
There is a GET and POST on '/v1/notes' which will get all notes and submit new notes, respectively. On '/v1/notes/{note_id}' there is a GET, PUT, and DELETE, which gets a note, modifies an exisiting note, and deletes a note, respectively.

The API Gateway sends all the requests the Lambda which handles all the above logic.

## How to build and deploy

1. Get an AWS account if you don't already have one https://aws.amazon.com/
1. Install [npm](https://www.npmjs.com/get-npm)
1. Install [.NET Core](https://dotnet.microsoft.com/download)
1. Install [AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-install.html).
1. Install AWS CDK command line tool ```npm install -g aws-cdk```. Morei nformation at [AWS CDK docs](https://awslabs.github.io/aws-cdk/getting-started.html).
1. Setup an AWS IAM user that has AdministratorAccess policy attatched. Make sure to create it with programmatic access.
1. ```aws configure``` and put in the access key id and secret access key given by AWS on creation of the IAM user.
1. ```cd NotesBackendLambda/src/NotesBackendLambda```
1. ```dotnet build```
1. ```dotnet publish -c Release -o out```
1. ```cd ../../../CDKInfra```
1. ```npm run build```
1. ```cdk deploy```

You'll see your API Gateway endpoint url in the terminal/cmd after the deployment has finished. Or you can logon to your AWS account and see it there.

