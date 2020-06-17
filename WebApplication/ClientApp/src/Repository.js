import axios from 'axios';

const AuthorizationHeader = 'Authorization';

/** Kind-of data layer, which hides all interaction with backend. */
class Repository {

    /** Get list of projects */
    async loadProjects() {
        const response = await axios.get("/projects");
        return response.data;
    }
    /** Get project parameters */
    async loadParameters(projectName) {
        const response = await axios.get("/parameters/" + projectName);
        return response.data;
    }

    /** Get the information if we should show the strip informing customer that parameters changed */
    async loadShowParametersChanged() {
        const response = await axios.get("/showParametersChanged");
        return response.data;
    }

    /** Send to server if we want to enable/disable permanently showing of  paramteres change banner */
    async sendShowParametersChanged(value) {
        const response = await axios.post("/showParametersChanged", value, {
            headers: {
                'content-type': 'application/json'
            }
        });
        return response.data;
    }

    /** Load users profile */
    async loadProfile() {
        const response = await axios.get("/login/profile");
        return response.data;
    }

    /**Uploads package to the server */
    async uploadPackage(form) {
        console.log(form);
    }

    setAccessToken(accessToken) {
        axios.defaults.headers.common[AuthorizationHeader] = accessToken;
    }

    forgetAccessToken() {
        delete axios.defaults.headers.common[AuthorizationHeader];
    }
}

/** Singleton with repo */
export default new Repository();