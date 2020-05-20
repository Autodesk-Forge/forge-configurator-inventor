import * as signalR from '@aspnet/signalr';

class JobManager {
    constructor() {
        this.jobs = new Map();
    }

    async doJob(projectId, parameters, onStart, onComplete) {
        const connection = new signalR.HubConnectionBuilder()
        .withUrl('/signalr/connection')
        .configureLogging(signalR.LogLevel.Trace)
        .build();

        await connection.start();

        if (onStart)
            onStart();

        connection.on("onComplete", (id, updatedState) => {
            // stop connection
            connection.stop();

            if (onComplete)
                onComplete(id, updatedState);
        });

        await connection.invoke('CreateJob', projectId, parameters);
    }
}

const jobManager = new JobManager();

export function Jobs() {
  return jobManager;
}


