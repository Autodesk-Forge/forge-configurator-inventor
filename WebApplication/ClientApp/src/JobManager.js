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

    async startConnection() {
        const connection = new signalR.HubConnectionBuilder()
                            .withUrl('/signalr/connection')
                            .configureLogging(signalR.LogLevel.Warning)
                            .build();

        await connection.start();
        return connection;
    }

    async doUpdateJob(projectId, parameters, onStart, onComplete, onError) {
        const connection = await this.startConnection();

        if (onStart)
            onStart();

        connection.on("onComplete", (updatedState, stats) => {
            // stop connection
            connection.stop();

            if (onComplete)
                onComplete(updatedState, stats);
        });

        connection.on("onError", (jobId, reportUrl) => {
            connection.stop();

            if (onError)
                onError(jobId, reportUrl);
        });

        await connection.invoke('CreateUpdateJob', projectId, parameters, repo.getAccessToken());
    }


    async doAdoptJob(packageId, onStart, onComplete, onError) {
        const connection = await this.startConnection();

        if (onStart)
            onStart();

        connection.on("onComplete", (newProject, stats) => {
            // stop connection
            connection.stop();

            if (onComplete)
                onComplete(newProject,stats);
        });

        connection.on("onError", (jobId, errorData) => {
            connection.stop();

            if (onError)
                onError(jobId, errorData);
        });

        await connection.invoke('CreateAdoptJob', packageId, repo.getAccessToken());
    }

    /**
     * Generic way to generate a download and get URL to it.
     *
     * @param methodName SignalR method to call.
     * @param projectId  Project ID.
     * @param hash       Parameters hash.
     * @param onStart    Callback to be called on start. No arguments.
     * @param onSuccess  Callback to be called on success. Argument: url to the generated download.
     * @param onError    Callback to be called on error. Arguments: job ID, report url.
     * */
    async doDownloadJob(methodName, projectId, hash, key, onStart, onSuccess, onError) {

        const connection = await this.startConnection();

        if (onStart) onStart();

        connection.on("onComplete", (downloadUrl, stats) => {

            connection.stop();

            if (! downloadUrl && ! stats) {
                if (onError) onError("no outputs", "Downloads are not found");
                return;
            }

            if (onSuccess) {
                const token = repo.getAccessToken();
                if (token) {
                    downloadUrl += "/" + token;
                }
                onSuccess(downloadUrl, stats);
            }
        });

        connection.on("onError", (jobId, reportUrl) => {

            connection.stop();
            if (onError) onError(jobId, reportUrl);
        });

        if (key != null)
            await connection.invoke(methodName, projectId, hash, key, repo.getAccessToken());
        else
            await connection.invoke(methodName, projectId, hash, repo.getAccessToken());
    }

    async doDrawingExportJob(projectId, hash, drawingKey, onStart, onComplete, onError) {
        const connection = await this.startConnection();

        if (onStart)
            onStart();

        connection.on("onComplete", (drawingUrl, stats) => {
            // stop connection
            connection.stop();

            if (onComplete) {
                onComplete(drawingUrl, stats);
            }
        });

        connection.on("onError", (jobId, reportUrl) => {
            connection.stop();

            if (onError)
                onError(jobId, reportUrl);
        });

        await connection.invoke('CreateDrawingPdfJob', projectId, hash, drawingKey, repo.getAccessToken());
    }
}

const jobManager = new JobManager();

export function Jobs() {
  return jobManager;
}


