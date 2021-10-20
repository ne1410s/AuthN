import * as express from 'express';
import * as path from 'path';

const expr_api = express();

// Static resources
expr_api.get('/style.css', (q, r) => r.sendFile(path.resolve(__dirname, '../ui/style.css')));
expr_api.get('/loading.svg', (q, r) => r.sendFile(path.resolve(__dirname, '../ui/loading.svg')));
expr_api.get('/main.js', (q, r) => r.sendFile(path.resolve(__dirname, '../ui/main.js')));
expr_api.get('/', (q, r) => r.sendFile(path.resolve(__dirname, '../ui/pages/index.html')));
expr_api.get('/login', (q, r) => r.sendFile(path.resolve(__dirname, '../ui/pages/login.html')));
expr_api.get('/members', (q, r) => r.sendFile(path.resolve(__dirname, '../ui/pages/members.html')));

// Start!
expr_api.listen(8081, () => {
  console.log(`Listening on http://localhost:8081`);
});
