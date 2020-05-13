import * as signalR from '@aspnet/signalr';

class JobManager {
    constructor() {
        this.jobs = new Map();
    }

    async doJob(jobCallback, onComplete) {

        let jobId = null;
        // eslint-disable-next-line prefer-const
        let jobInfo = {
            state: "notStarted",
            connectionId: null,
            connection: null,
            onStart: function(id) { jobId=id; },
            onComplete: onComplete
        };

        await connect(jobInfo);
        jobCallback(jobInfo.connectionId);

        // store job
        this.jobs.set(jobId, jobInfo);
    }
}

const jobManager = new JobManager();

export function Jobs() {
  return jobManager;
}

function stopConnection(jobInfo) {
    jobInfo.connection.stop();
    jobInfo.connectionId = null;
    jobInfo.connection = null;
}

async function connect(jobInfo) {
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

        jobInfo.connection.on("onStarted", function (id) {
            if (jobInfo.onStart)
                jobInfo.onStart(id);
        });

        jobInfo.connection.on("onComplete", function (id, data) {
            // stop connection
            stopConnection(jobInfo);

            if (jobInfo.onComplete)
                jobInfo.onComplete(id,data);
        });
    } catch (error) {
        // eslint-disable-next-line no-console
        console.error('Failed to call updateModelWithParameters :' + error);
    }
}
