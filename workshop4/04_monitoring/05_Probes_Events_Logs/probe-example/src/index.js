const express = require('express');
const fs = require('node:fs');

// Define sleep function
const sleep = ms => new Promise(resolve => {
    console.log(`Sleeping for ${ms} milliseconds`);
    setTimeout(resolve, ms);
})

const SLEEP_TIME = 10000     // 10000 milliseconds
const HEALTHZ_TIME = 40000   // 40000 milliseconds

async function startMicroservice()
{
    console.log('Microservice started.');

    // Create an Express instance
    const app = express();
    console.log('Created express web server.');

    // Set timestamp "startupTimestamp" of when the microservice started
    const startupTimestamp = new Date();
    console.log(`Set startupTimestamp to ${startupTimestamp.toLocaleTimeString('sv-SE')}`);

    // This is a normal HTTP Get route (path) for the microservice (part of the microservice's functionality)
    app.get('/', async (req, res) => {
        res.sendStatus(200);
    });

    // Respond to HTTP GET requests on route (path) "/healthz" to indicate "alive" (this is what the livenessProbe checks)
    app.get('/healthz', async (req, res) => {
        const current = new Date();
        console.log(`Route /healthz hit at time ${current.toLocaleTimeString('sv-SE')}, Elapsed seconds since startup: ${(current - startupTimestamp)/1000}`);

        if (current - startupTimestamp < HEALTHZ_TIME) {
            console.log('Route /healthz returning status 200');
            res.sendStatus(200); // If within 40 seconds of the microservice's "startupTimestamp", return status "200" (ok)
        } else {
            console.log('Route /healthz returning status 500');
            res.sendStatus(500); // If not within 40 seconds of the microservice's "startupTimestamp", return status "500" (internal server error)
        }
    });

    // Wait (sleep) for 10 seconds to simulate startup time (initializing the microservice)
    await sleep(SLEEP_TIME);

    // Write file "/tmp/started" to indicate "started" (this is what the startupProbe checks)
    try {
        fs.writeFileSync('/tmp/started', 'started');
        console.log('Wrote file /tmp/started.');
    } catch(err) {
        console.error(err);
    }

    // Wait (sleep) for 10 seconds to simulate time between startup and readyness
    await sleep(SLEEP_TIME);

    // Start Express Web Server listening on port 3000 to indicate "ready" (this is what the readinessProbe checks)
    const port = process.env.PORT || 3000;
    app.listen(port, (err) => {
        if (err) console.error(err)
        else console.log(`Started listening on port ${port}.`)
    });
}

startMicroservice()