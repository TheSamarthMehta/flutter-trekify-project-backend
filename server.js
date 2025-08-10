// server.js

// --- Import Dependencies ---
const express = require('express');
const mongoose = require('mongoose');
const dotenv = require('dotenv');
const cors = require('cors');

// --- Initializations ---
dotenv.config();
const app = express();

// --- Middleware Configuration ---
app.use(cors());
app.use(express.json());

// --- Database Connection ---
mongoose
  .connect(process.env.MONGO_URI, {
    useNewUrlParser: true,
    useUnifiedTopology: true,
  })
  .then(() => console.log('✅ MongoDB Connected Successfully'))
  .catch((err) => console.error('❌ MongoDB Connection Error:', err));

// --- API Routes ---
app.use('/api/auth', require('./routes/auth'));
app.use('/api/data', require('./routes/data')); 
// ✅ ADD THIS LINE to include your new onboarding route
app.use('/api/onboarding', require('./routes/onboarding'));

// A simple base route to confirm the API is running
app.get('/', (req, res) => {
  res.send('Trekify API (Auth & Data) is up and running...');
});

// --- Start the Server ---
const PORT = process.env.PORT || 5000;
app.listen(PORT, () => console.log(`🚀 Server started on port ${PORT}`));