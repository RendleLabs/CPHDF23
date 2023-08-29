import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
  stages: [
    { duration: '10s', target: 5 },
  ]
};

export default function() {
  
  const page = Math.floor(Math.random() * 100);

  const response = http.get(`https://localhost:5001/api/sightings?page=${page}`);

  console.log(response.status);

  // sleep(1);
}
