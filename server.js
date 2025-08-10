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

// ✅ MERGED: Use both the authentication routes and the new data routes
app.use('/api/auth', require('./routes/auth'));
app.use('/api/data', require('./routes/data')); // This line is new

// A simple base route to confirm the API is running
app.get('/', (req, res) => {
  res.send('Trekify API (Auth & Data) is up and running...');
});

// --- Start the Server ---
const PORT = process.env.PORT || 5000;
app.listen(PORT, () => console.log(`🚀 Server started on port ${PORT}`));