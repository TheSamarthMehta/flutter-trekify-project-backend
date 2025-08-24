const mongoose = require('mongoose');

const userSchema = new mongoose.Schema({
  email: {
    type: String,
    required: true,
    unique: true,
    lowercase: true,
    trim: true
  },
  password: {
    type: String,
    required: true,
    minlength: 6
  },
  name: {
    type: String,
    required: true,
    trim: true
  },
  preferences: {
    trekTypes: [{
      type: String,
      enum: ['day_hike', 'weekend_trek', 'expedition', 'pilgrimage', 'adventure_sport']
    }],
    difficultyLevels: [{
      type: String,
      enum: ['easy', 'moderate', 'difficult', 'extreme']
    }],
    seasons: [{
      type: String,
      enum: ['winter', 'summer', 'monsoon', 'post_monsoon']
    }],
    states: [{
      type: String
    }]
  },
  isProfileComplete: {
    type: Boolean,
    default: false
  }
}, {
  timestamps: true
});

// Index for email lookups
userSchema.index({ email: 1 });

module.exports = mongoose.model('User', userSchema);
