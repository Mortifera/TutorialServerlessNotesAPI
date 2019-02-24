import cdk = require('@aws-cdk/cdk');
import ddb = require('@aws-cdk/aws-dynamodb');
import lambda = require('@aws-cdk/aws-lambda');
import apig = require('@aws-cdk/aws-apigateway');
import { Output } from '@aws-cdk/cdk';

export class WebAPIStack extends cdk.Stack {
    constructor(scope: cdk.App, id: string, props?: cdk.StackProps) {
        super(scope, id, props);

        const notesTable = new ddb.Table(this, "NotesTable", {
            billingMode: ddb.BillingMode.PayPerRequest,
            partitionKey: {
                name: 'NoteId',
                type: ddb.AttributeType.String
            }
        });

        const lamdbaFunction = new lambda.Function(this, "BackendLambdaFunction", {
            code: new lambda.AssetCode("../NotesBackendLambda/src/out"),
            handler: "NotesBackendLambda::NotesBackendLambda.Function::FunctionHandler",
            timeout: 30,
            runtime: lambda.Runtime.DotNetCore21,
            environment: {
                "NOTES_TABLE_NAME": notesTable.tableName 
            }
        });

        notesTable.grantFullAccess(lamdbaFunction.role);

        const notesBackend = new apig.LambdaIntegration(lamdbaFunction, {
            proxy: true
        })
        const api = new apig.RestApi(this, 'NotesAPI', {
            defaultIntegration: notesBackend
        });

        const v1ApiResource = api.root.addResource('v1');

        const notes = v1ApiResource.addResource('notes');
        notes.addMethod('GET');
        notes.addMethod('POST');

        const note = notes.addResource('{note_id}');
        note.addMethod('GET');
        note.addMethod('PUT');
        note.addMethod('DELETE');

        new Output(this, "APIEndpoint", {
            value: api.url
        });
    }
}
