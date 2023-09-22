import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
  stages: [
    { duration: '10s', target: 10 },
    { duration: '30s', target: 100 },
    { duration: '10s', target: 0 }
  ]
};

export default function() {
  
  const page = Math.floor(Math.random() * 100);

  http.get(`http://localhost:5000/api/sightings?page=${page}`);

  // sleep(1);
}
