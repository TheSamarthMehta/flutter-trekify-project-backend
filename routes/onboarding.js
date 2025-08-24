const express = require('express');
const auth = require('../middleware/auth');
const User = require('../models/User');

const router = express.Router();

// @route   POST /api/onboarding/preferences
// @desc    Save user preferences during onboarding
// @access  Private
router.post('/preferences', auth, async (req, res) => {
  try {
    const { trekTypes, difficultyLevels, seasons, states } = req.body;

    // Validation
    if (!trekTypes || !difficultyLevels || !seasons || !states) {
      return res.status(400).json({
        success: false,
        message: 'Please provide all preference categories'
      });
    }

    // Validate enum values
    const validTrekTypes = ['day_hike', 'weekend_trek', 'expedition', 'pilgrimage', 'adventure_sport'];
    const validDifficultyLevels = ['easy', 'moderate', 'difficult', 'extreme'];
    const validSeasons = ['winter', 'summer', 'monsoon', 'post_monsoon'];

    const invalidTrekTypes = trekTypes.filter(type => !validTrekTypes.includes(type));
    const invalidDifficulties = difficultyLevels.filter(level => !validDifficultyLevels.includes(level));
    const invalidSeasons = seasons.filter(season => !validSeasons.includes(season));

    if (invalidTrekTypes.length > 0) {
      return res.status(400).json({
        success: false,
        message: `Invalid trek types: ${invalidTrekTypes.join(', ')}`
      });
    }

    if (invalidDifficulties.length > 0) {
      return res.status(400).json({
        success: false,
        message: `Invalid difficulty levels: ${invalidDifficulties.join(', ')}`
      });
    }

    if (invalidSeasons.length > 0) {
      return res.status(400).json({
        success: false,
        message: `Invalid seasons: ${invalidSeasons.join(', ')}`
      });
    }

    // Update user preferences
    const user = await User.findByIdAndUpdate(
      req.user._id,
      {
        preferences: {
          trekTypes,
          difficultyLevels,
          seasons,
          states
        },
        isProfileComplete: true
      },
      { new: true, runValidators: true }
    ).select('-password');

    res.json({
      success: true,
      message: 'Preferences saved successfully',
      data: {
        user: {
          id: user._id,
          email: user.email,
          name: user.name,
          isProfileComplete: user.isProfileComplete,
          preferences: user.preferences
        }
      }
    });

  } catch (error) {
    console.error('Save preferences error:', error);
    res.status(500).json({
      success: false,
      message: 'Server error while saving preferences'
    });
  }
});

// @route   GET /api/onboarding/preferences
// @desc    Get user preferences
// @access  Private
router.get('/preferences', auth, async (req, res) => {
  try {
    const user = await User.findById(req.user._id).select('-password');
    
    res.json({
      success: true,
      data: {
        preferences: user.preferences || {
          trekTypes: [],
          difficultyLevels: [],
          seasons: [],
          states: []
        },
        isProfileComplete: user.isProfileComplete
      }
    });

  } catch (error) {
    console.error('Get preferences error:', error);
    res.status(500).json({
      success: false,
      message: 'Server error while fetching preferences'
    });
  }
});

// @route   GET /api/onboarding/options
// @desc    Get all available options for onboarding
// @access  Public
router.get('/options', async (req, res) => {
  try {
    const options = {
      trekTypes: [
        { value: 'day_hike', label: 'Day Hike' },
        { value: 'weekend_trek', label: 'Weekend Trek' },
        { value: 'expedition', label: 'Expedition' },
        { value: 'pilgrimage', label: 'Pilgrimage' },
        { value: 'adventure_sport', label: 'Adventure Sport' }
      ],
      difficultyLevels: [
        { value: 'easy', label: 'Easy' },
        { value: 'moderate', label: 'Moderate' },
        { value: 'difficult', label: 'Difficult' },
        { value: 'extreme', label: 'Extreme' }
      ],
      seasons: [
        { value: 'winter', label: 'Winter (Dec-Feb)' },
        { value: 'summer', label: 'Summer (Mar-May)' },
        { value: 'monsoon', label: 'Monsoon (Jun-Sep)' },
        { value: 'post_monsoon', label: 'Post Monsoon (Oct-Nov)' }
      ]
    };

    res.json({
      success: true,
      data: options
    });

  } catch (error) {
    console.error('Get options error:', error);
    res.status(500).json({
      success: false,
      message: 'Server error while fetching options'
    });
  }
});

module.exports = router;
