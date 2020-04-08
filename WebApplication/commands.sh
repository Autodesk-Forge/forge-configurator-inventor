#!/bin/sh -e
/opt/aws/amazon-cloudwatch-agent/bin/start-amazon-cloudwatch-agent &
dotnet SalesDemoToolApp.dll