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

    async doRFAJob(projectId, hash, onStart, onComplete) {
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

        await connection.invoke('CreateRFAJob', projectId, hash, repo.getAccessToken());
    }
}

const jobManager = new JobManager();

export function Jobs() {
  return jobManager;
}


