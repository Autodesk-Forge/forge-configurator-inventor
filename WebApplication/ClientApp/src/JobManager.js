import * as signalR from '@aspnet/signalr';

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

    async doUpdateJob(projectId, parameters, onStart, onComplete) {
        const connection = await this.startConnection();

        if (onStart)
            onStart();

        connection.on("onComplete", (id, updatedState) => {
            // stop connection
            connection.stop();

            if (onComplete)
                onComplete(id, updatedState);
        });

        await connection.invoke('CreateUpdateJob', projectId, parameters);
    }

    async doRFAJob(projectId, onStart, onComplete) {
        const connection = await this.startConnection();

        if (onStart)
            onStart();

        connection.on("onComplete", (id) => {
            // stop connection
            connection.stop();

            if (onComplete)
                onComplete(id);
        });

        await connection.invoke('CreateRFAJob', projectId);
    }
}

const jobManager = new JobManager();

export function Jobs() {
  return jobManager;
}


