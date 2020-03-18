import axios from 'axios';

/** Kind-of data layer, which hides all interaction with backend. */
class Repository {

    /** Get list of projects */
    async loadProjects() {
        try {

            const response = await axios.get("/Project");
            console.log(response);

            return response.data;
        } catch (error) {

            console.log(`Project loading error: ${error}`);
            return null;
        }
    }
}

/** Singleton with repo */
const repo = new Repository();

/** Get instance of the repository */
export function getRepo() {
    return repo;
}
