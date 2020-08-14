/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

import * as signalR from '@aspnet/signalr';
import repo from './Repository';

class JobManager {
    constructor() {
        this.jobs = new Map();
    }

    async startConnection() {
        const connection = new signalR.HubConnectionBuilder()
        .withUrl('/signalr/connection')
        .configureLogging(signalR.LogLevel.Trace)
        .build();

        await connection.start();
        return connection;
    }

    async doUpdateJob(projectId, parameters, onStart, onComplete, onError) {
        const connection = await this.startConnection();

        if (onStart)
            onStart();

        connection.on("onComplete", (updatedState) => {
            // stop connection
            connection.stop();

            if (onComplete)
                onComplete(updatedState);
        });

        connection.on("onError", (jobId, reportUrl) => {
            connection.stop();

            if (onError)
                onError(jobId, reportUrl);
        });

        await connection.invoke('CreateUpdateJob', projectId, parameters, repo.getAccessToken());
    }

    async doRFAJob(projectId, hash, onStart, onComplete, onError) {
        const connection = await this.startConnection();

        if (onStart)
            onStart();

        connection.on("onComplete", (rfaUrl) => {
            // stop connection
            connection.stop();

            if (onComplete) {
                if (repo.getAccessToken()) {
                    rfaUrl += "/" + repo.getAccessToken();
                }
                onComplete(rfaUrl);
            }
        });

        connection.on("onError", (jobId, reportUrl) => {
            connection.stop();

            if (onError)
                onError(jobId, reportUrl);
        });

        await connection.invoke('CreateRFAJob', projectId, hash, repo.getAccessToken());
    }

    async doAdoptJob(packageId, onStart, onComplete, onError) {
        const connection = await this.startConnection();

        if (onStart)
            onStart();

        connection.on("onComplete", (newProject) => {
            // stop connection
            connection.stop();

            if (onComplete)
                onComplete(newProject);
        });

        connection.on("onError", (jobId, reportUrl) => {
            connection.stop();

            if (onError)
                onError(jobId, reportUrl);
        });

        await connection.invoke('CreateAdoptJob', packageId, repo.getAccessToken());
    }

    async doDrawingDownloadJob(projectId, hash, onStart, onComplete, onError) {
        const connection = await this.startConnection();

        if (onStart)
            onStart();

        connection.on("onComplete", (drawingUrl) => {
            // stop connection
            connection.stop();

            if (onComplete) {
                if (repo.getAccessToken()) {
                    drawingUrl += "/" + repo.getAccessToken();
                }
                onComplete(drawingUrl);
            }
        });

        connection.on("onError", (jobId, reportUrl) => {
            connection.stop();

            if (onError)
                onError(jobId, reportUrl);
        });

        await connection.invoke('CreateDrawingDownloadJob', projectId, hash, repo.getAccessToken());
    }

    async doDrawingExportJob(projectId, hash, onStart, onComplete, onError) {
        const connection = await this.startConnection();

        if (onStart)
            onStart();

        connection.on("onComplete", (drawingUrl) => {
            // stop connection
            connection.stop();

            if (onComplete) {
                onComplete(drawingUrl);
            }
        });

        connection.on("onError", (jobId, reportUrl) => {
            connection.stop();

            if (onError)
                onError(jobId, reportUrl);
        });

        await connection.invoke('CreateExportDrawingJob', projectId, hash, repo.getAccessToken());
    }
}

const jobManager = new JobManager();

export function Jobs() {
  return jobManager;
}


