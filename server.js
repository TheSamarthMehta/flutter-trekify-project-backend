const express = require('express');
const mongoose = require('mongoose');
const cors = require('cors');
const path = require('path');
require('dotenv').config();

const app = express();

// Middleware
app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Serve static files (images)
app.use('/images', express.static(path.join(__dirname, 'Images')));

// MongoDB connection
const connectDB = async () => {
  try {
    if (process.env.MONGO_URI) {
      const conn = await mongoose.connect(process.env.MONGO_URI, {
        useNewUrlParser: true,
        useUnifiedTopology: true,
      });
      console.log(`✅ MongoDB Connected: ${conn.connection.host}`);
    } else {
      console.log('⚠️  No MongoDB URI found - using local data only');
    }
  } catch (error) {
    console.warn('❌ MongoDB connection failed:', error.message);
  }
};

// Connect to database
connectDB();

// Routes
app.use('/api/auth', require('./routes/auth'));
app.use('/api/data', require('./routes/data'));

// Health check route
app.get('/', (req, res) => {
  res.json({ 
    success: true,
    message: 'Trekify API Running',
    version: '1.0.0',
    environment: process.env.NODE_ENV || 'development'
  });
});

// 404 handler
app.use('*', (req, res) => {
  res.status(404).json({ 
    success: false, 
    message: 'Route not found' 
  });
});

// Error handling middleware
app.use((err, req, res, next) => {
  console.error('❌ Error:', err.message);
  res.status(500).json({ 
    success: false, 
    message: 'Server error'
  });
});

const PORT = process.env.PORT || 5000;

app.listen(PORT, () => {
  console.log('🚀 Trekify Backend Started');
  console.log(`📡 Server: http://localhost:${PORT}`);
  console.log(`🌍 Environment: ${process.env.NODE_ENV || 'development'}`);
  console.log(`🔑 JWT Secret: ${process.env.JWT_SECRET ? 'Configured' : 'Not Set'}`);
  console.log(`💾 Database: ${process.env.MONGO_URI ? 'MongoDB' : 'Local Data'}`);
  console.log('✨ Ready to serve requests\n');
});
