import * as signalR from '@aspnet/signalr';

class JobManager {
    constructor() {
        this.jobs = new Map();
    }

    async doJob(projectId, data, onStart, onComplete) {
        const connection = new signalR.HubConnectionBuilder()
        .withUrl('/signalr/connection')
        .configureLogging(signalR.LogLevel.Trace)
        .build();

        await connection.start();

        if (onStart)
            onStart();

        await connection.invoke('CreateJob', projectId, JSON.stringify(data));

        connection.on("onComplete", (id) => {
            // stop connection
            connection.stop();

            if (onComplete)
                onComplete(id);
        });
    }
}

const jobManager = new JobManager();

export function Jobs() {
  return jobManager;
}


