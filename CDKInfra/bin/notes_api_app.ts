#!/usr/bin/env node
import cdk = require('@aws-cdk/cdk');
import { WebAPIStack } from '../lib/notes_api_stack';

const app = new cdk.App();
new WebAPIStack(app, 'NotesAPIStack');
app.run();
