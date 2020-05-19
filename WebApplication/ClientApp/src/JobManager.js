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

        await connection.invoke('CreateJob', projectId, parameters);

        connection.on("onComplete", (id, updatedState) => {
            // stop connection
            connection.stop();

            if (onComplete)
                onComplete(id, updatedState);
        });
    }
}

const jobManager = new JobManager();

export function Jobs() {
  return jobManager;
}


