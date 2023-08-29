import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
  stages: [
    { duration: '10s', target: 10 },
    { duration: '1m', target: 100 },
    { duration: '30s', target: 0 }
  ]
};

export default function() {
  
  const page = Math.floor(Math.random() * 100);

  http.get(`https://localhost:5001/api/sightings?page=${page}`);

  // sleep(1);
}
