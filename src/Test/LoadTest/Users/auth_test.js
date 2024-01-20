import http from 'k6/http';
import { sleep, check } from 'k6';
import encoding from 'k6/encoding';

const loginUrl = 'https://localhost:7093/api/User/login';

export const options = {
  // // A number specifying the number of VUs to run concurrently.
  // vus: 10,
  // // A string specifying the total duration of the test run.
  // duration: '30s',
};

// The function that defines VU logic.
//
// See https://grafana.com/docs/k6/latest/examples/get-started-with-k6/ to learn more
// about authoring k6 scripts.
//
export default function() {
  // Correct username and password
  var payload = JSON.stringify({
    username: 'admin',
    password: 'Qwerty123'
  });
  
  const params = {
    headers: {
      'Content-Type' : 'application/json',
    },
  };

  var res = http.post(loginUrl, payload, params);
  console.log('Correct Test : ' + res.body);
  check(res, {
    'response code was 200': (res) => res.status == 200,
  });

  // Incorrect username
  payload = JSON.stringify({
    username: 'adminsss',
    password: 'Qwerty123'
  });

  res = http.post(loginUrl, payload, params);
  check(res, {
    'response code was 500 : user not found': (res) => res.status == 500,
  });

  // Incorrect password
  payload = JSON.stringify({
    username: 'admin',
    password: 'qwerty123'
  });

  res = http.post(loginUrl, payload, params);
  console.log('Incorrect password Test : ' + res.body);
  check(res, {
    'response code was 500 : incorrect password': (res) => res.status == 500,
  });
}
