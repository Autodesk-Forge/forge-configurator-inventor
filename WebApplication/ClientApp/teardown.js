const axios = require('axios');

module.exports = function() {
   console.log('Tear down');

   process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

   axios.get('https://localhost:5001/ClearSelf')
   .then(response => {
      console.log(response.data);
   })
   .catch(error => {
     console.log(error);
   });
}