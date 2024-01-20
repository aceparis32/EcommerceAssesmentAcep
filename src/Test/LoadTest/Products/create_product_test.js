import http from 'k6/http';
import { sleep, check } from 'k6';
import encoding from 'k6/encoding';

var username = 'admin';
var password = 'Qwerty123';
const loginUrl = 'https://localhost:7093/api/User/login';
const createProductUrl = 'https://localhost:7093/api/Product';

export default function() {
  var payload = JSON.stringify({
    username: username,
    password: password
  });
  
  var params = {
    headers: {
      'Content-Type' : 'application/json',
    },
  };

  var res = http.post(loginUrl, payload, params);

  payload = JSON.stringify({
    name: 'test admin product',
    description : 'test of creating product by admin',
    price : 10000000,
    rating : 5.2,
    stock : 10,
    brand : 'test',
    category : 'test'
  });

  params = {
    headers: {
      'Content-Type' : 'application/json',
      'Authorization' : 'bearer ' + res.json().token,
    },
  };

  res = http.post(createProductUrl, payload, params);
  check(res, {
    'correct response code was 200': (res) => res.status == 200,
  });

  // test by seller
  username = 'seller1';

  payload = JSON.stringify({
    username: username,
    password: password
  });
  
  params = {
    headers: {
      'Content-Type' : 'application/json',
    },
  };

  res = http.post(loginUrl, payload, params);

  // create product test
  payload = JSON.stringify({
    name: 'test seller product',
    description : 'test of creating product by seller',
    price : 10000000,
    rating : 5.2,
    stock : 10,
    brand : 'test',
    category : 'test'
  });

  params = {
    headers: {
      'Content-Type' : 'application/json',
      'Authorization' : 'bearer ' + res.json().token,
    },
  };

  res = http.post(createProductUrl, payload, params);
  check(res, {
    'correct response code was 200': (res) => res.status == 200,
  });

  // test by customer
  username = 'customer1';

  payload = JSON.stringify({
    username: username,
    password: password
  });
  
  params = {
    headers: {
      'Content-Type' : 'application/json',
    },
  };

  res = http.post(loginUrl, payload, params);

  // create product test
  payload = JSON.stringify({
    name: 'test customer product',
    description : 'test of creating product by customer',
    price : 10000000,
    rating : 5.2,
    stock : 10,
    brand : 'test',
    category : 'test'
  });

  params = {
    headers: {
      'Content-Type' : 'application/json',
      'Authorization' : 'bearer ' + res.json().token,
    },
  };

  res = http.post(createProductUrl, payload, params);
  check(res, {
    'correct response code was 500': (res) => res.status == 500,
  });
}
