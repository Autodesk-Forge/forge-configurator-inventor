import * as signalR from '@aspnet/signalr';

class JobManager {
    constructor() {
        this.jobs = new Map();
    }

    async doJob(jobCallback, onComplete) {

        let jobId = null;
        const jobInfo = {
            state: "notStarted",
            connectionId: null,
            connection: null,
            onStart: (id, data) => {
                // eslint-disable-next-line no-console
                console.log('job ' + id + ' started : ' + data);
                jobId = id;
            },
            onComplete: onComplete
        };

        await this.connect(jobInfo);
        jobCallback(jobInfo.connectionId);

        // store job
        this.jobs.set(jobId, jobInfo);
    }

    stopConnection(jobInfo) {
        jobInfo.connection.stop();
        jobInfo.connectionId = null;
        jobInfo.connection = null;
    }

    async connect(jobInfo) {
        if (jobInfo != null &&
            jobInfo.connection != null && jobInfo.connection.connectionState) {
            return;
        }

        try {

            jobInfo.connection = new signalR.HubConnectionBuilder()
            .withUrl('/signalr/connection')
            .configureLogging(signalR.LogLevel.Information)
            .build();

            await jobInfo.connection.start();
            const id = await jobInfo.connection.invoke('getConnectionId');
            jobInfo.connectionId = id;

            jobInfo.connection.on("onStarted", (id, data) => {
                if (jobInfo.onStart)
                    jobInfo.onStart(id, data);
            });

            jobInfo.connection.on("onComplete", (id, data) => {
                // stop connection
                this.stopConnection(jobInfo);

                if (jobInfo.onComplete)
                    jobInfo.onComplete(id,data);
            });
        } catch (error) {
            // eslint-disable-next-line no-console
            console.error('Failed to call updateModelWithParameters :' + error);
        }
    }
}

const jobManager = new JobManager();

export function Jobs() {
  return jobManager;
}


