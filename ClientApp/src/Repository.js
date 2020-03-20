import axios from 'axios';

/** Kind-of data layer, which hides all interaction with backend. */
class Repository {

    /** Get list of projects */
    async loadProjects() {
        const response = await axios.get("/Project");
        return response.data;
    }
}

/** Singleton with repo */
export default new Repository();