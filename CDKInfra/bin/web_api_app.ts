#!/usr/bin/env node
import cdk = require('@aws-cdk/cdk');
import { WebAPIStack } from '../lib/web_api_stack';

const app = new cdk.App();
new WebAPIStack(app, 'WebAPIStack');
app.run();
