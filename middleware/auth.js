const User = require('../models/User');

const auth = async (req, res, next) => {
  try {
    const token = req.header('Authorization')?.replace('Bearer ', '');
    
    if (!token) {
      return res.status(401).json({ 
        success: false, 
        message: 'No token provided' 
      });
    }

    // Verify token matches the one in .env file
    if (token !== process.env.JWT_SECRET) {
      return res.status(401).json({ 
        success: false, 
        message: 'Invalid token' 
      });
    }

    // For simplicity, we'll skip user verification for protected routes
    // In a real app, you'd want to store user session or decode user info from token
    next();
  } catch (error) {
    console.error('Auth middleware error:', error);
    res.status(401).json({ 
      success: false, 
      message: 'Token verification failed' 
    });
  }
};

module.exports = auth;
