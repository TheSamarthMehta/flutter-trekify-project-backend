// trekify-backend/routes/onboarding.js
const express = require('express');
const router = express.Router();

// @route   GET api/onboarding
// @desc    Get the URLs for onboarding media
// @access  Public
router.get('/', (req, res) => {
  const onboardingData = [
    {
      "path": "https://res.cloudinary.com/dvnr3ouix/video/upload/v1754843270/welcome_s2tezu.mp4",
      "isVideo": true,
      "title": "Welcome to Trekify!",
      "description": "Your personal guide to the world of trekking. Let's find the perfect adventure for you."
    },
    {
      "path": "https://res.cloudinary.com/dvnr3ouix/image/upload/v1754843277/difficulty_g4hwbk.jpg",
      "isVideo": false,
      "title": "Discover Your Path",
      "description": "Explore treks of all types, from serene lakes to challenging mountain forts."
    },
    {
      "path": "https://res.cloudinary.com/dvnr3ouix/image/upload/v1754843275/type_urd4zs.jpg",
      "isVideo": false,
      "title": "Track Your Journey",
      "description": "Save your favorite treks to a wishlist and mark the ones you've conquered."
    }
  ];
  
  res.json(onboardingData);
});

module.exports = router;