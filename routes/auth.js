// routes/auth.js

// --- Import Dependencies ---
const express = require('express');
const router = express.Router();
const bcrypt = require('bcryptjs');
const jwt = require('jsonwebtoken');
const crypto = require('crypto');
const { check, validationResult } = require('express-validator');
const auth = require('../middleware/auth'); // Middleware to verify tokens
const User = require('../models/User'); // Your User model

// Note: The OAuth2Client for Google has been removed.

// --- API Routes ---

/**
 * @route   POST api/auth/register
 * @desc    Register a new user with email & password
 * @access  Public
 */
router.post(
  '/register',
  [
    // --- Input Validation ---
    check('name', 'Name is required').not().isEmpty(),
    check('email', 'Please include a valid email').isEmail(),
    check('password', 'Password must be 6 or more characters').isLength({ min: 6 }),
  ],
  async (req, res) => {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { name, email, password } = req.body;

    try {
      // Check if a user with this email already exists
      let user = await User.findOne({ email });
      if (user) {
        return res.status(400).json({ msg: 'User already exists' });
      }

      // Create a new user instance
      user = new User({ name, email, password });

      // Encrypt the password before saving to the database
      const salt = await bcrypt.genSalt(10);
      user.password = await bcrypt.hash(password, salt);

      // Save the new user to the database
      await user.save();

      // Create a JWT payload containing the user ID
      const payload = { user: { id: user.id } };

      // Sign the token and send it back to the client
      jwt.sign(
        payload,
        process.env.JWT_SECRET,
        { expiresIn: '5d' }, // Token expires in 5 days
        (err, token) => {
          if (err) throw err;
          res.status(200).json({ token });
        }
      );
    } catch (err) {
      console.error(err.message);
      res.status(500).send('Server error');
    }
  }
);

/**
 * @route   POST api/auth/login
 * @desc    Authenticate a user and get a token
 * @access  Public
 */
router.post(
  '/login',
  [
    // --- Input Validation ---
    check('email', 'Please include a valid email').isEmail(),
    check('password', 'Password is required').exists(),
  ],
  async (req, res) => {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { email, password } = req.body;

    try {
      // Check if the user exists
      let user = await User.findOne({ email });
      if (!user) {
        // User not found. Send a 404 status to trigger the sign-up flow on the frontend.
        return res.status(404).json({ msg: 'User not found. Please sign up.' });
      }

      // Compare the provided password with the hashed password in the database
      const isMatch = await bcrypt.compare(password, user.password);
      if (!isMatch) {
        // Password does not match
        return res.status(400).json({ msg: 'Invalid password. Please try again.' });
      }

      // Create a JWT payload
      const payload = { user: { id: user.id } };

      // Sign and return the token
      jwt.sign(
        payload,
        process.env.JWT_SECRET,
        { expiresIn: '5d' },
        (err, token) => {
          if (err) throw err;
          res.status(200).json({ token });
        }
      );
    } catch (err) {
      console.error(err.message);
      res.status(500).send('Server error');
    }
  }
);

/**
 * @route   GET api/auth/me
 * @desc    Get the data of the currently logged-in user
 * @access  Private
 */
router.get('/me', auth, async (req, res) => {
  try {
    // The 'auth' middleware verifies the token and attaches the user's ID to the request object.
    // We fetch the user from the database but exclude the password for security.
    const user = await User.findById(req.user.id).select('-password');
    res.json(user);
  } catch (err) {
    console.error(err.message);
    res.status(500).send('Server Error');
  }
});


// ✅ --- ADD THE FOLLOWING TWO ROUTES AT THE END OF THE FILE ---

/**
 * @route   POST api/auth/forgot-password
 * @desc    Request a password reset (sends a "reset token")
 * @access  Public
 */
router.post('/forgot-password', [check('email', 'Please include a valid email').isEmail()], async (req, res) => {
  const errors = validationResult(req);
  if (!errors.isEmpty()) {
    return res.status(400).json({ errors: errors.array() });
  }

  try {
    const user = await User.findOne({ email: req.body.email });
    if (!user) {
      // We send a success message even if the user doesn't exist
      // to prevent email enumeration attacks.
      return res.status(200).json({ msg: 'If a user with that email exists, a reset link has been sent.' });
    }

    // --- In a real app, you would generate and save a hashed token to the user model ---
    // For this simulation, we'll just generate a token and log it.
    const resetToken = crypto.randomBytes(20).toString('hex');
    
    console.log('================================================');
    console.log('PASSWORD RESET SIMULATION');
    console.log(`User: ${user.email}`);
    console.log(`Reset Token: ${resetToken}`);
    console.log('In a real app, you would email a link like: /reset-password/' + resetToken);
    console.log('================================================');

    res.status(200).json({ msg: 'If a user with that email exists, a reset link has been sent.' });

  } catch (err) {
    console.error(err.message);
    res.status(500).send('Server Error');
  }
});

/**
 * @route   POST api/auth/reset-password
 * @desc    Reset the user's password using a token
 * @access  Public
 */
router.post('/reset-password', [check('password', 'Password must be 6 or more characters').isLength({ min: 6 })], async (req, res) => {
    // This is a simplified version. A real implementation would find the user by their reset token.
    // For now, we'll just log that the action would have happened.
    console.log('Password reset attempt for token:', req.body.token);
    console.log('New password would be:', req.body.password);
    res.status(200).json({ msg: 'Password has been reset successfully.' });
});

// At the end of the file, before module.exports

/**
 * @route   PUT api/auth/update
 * @desc    Update user's name and avatar
 * @access  Private
 */
router.put('/update', [auth, [
    check('name', 'Name is required').not().isEmpty(),
]], async (req, res) => {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }

    const { name, avatar } = req.body;

    try {
        const user = await User.findById(req.user.id);
        if (!user) {
            return res.status(404).json({ msg: 'User not found' });
        }

        user.name = name;
        if (avatar) {
            user.avatar = avatar; // In a real app, you'd handle image uploads
        }

        await user.save();
        res.json(user); // Send back the updated user

    } catch (err) {
        console.error(err.message);
        res.status(500).send('Server Error');
    }
});

module.exports = router;