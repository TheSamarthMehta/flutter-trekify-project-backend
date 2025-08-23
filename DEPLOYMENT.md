# Deploying Trekify .NET Backend to Render

## Prerequisites
- GitHub account
- Render account (free tier available)
- MongoDB Atlas account (for cloud database)

## Step-by-Step Deployment Guide

### 1. Prepare MongoDB Database

1. **Create MongoDB Atlas Account**:
   - Go to [MongoDB Atlas](https://www.mongodb.com/atlas)
   - Create a free account and cluster

2. **Get Connection String**:
   - In Atlas dashboard, click "Connect"
   - Choose "Connect your application"
   - Copy the connection string (format: `mongodb+srv://username:password@cluster.mongodb.net/database`)

### 2. Push Code to GitHub

1. **Commit all changes**:
   ```bash
   git add .
   git commit -m "Add .NET backend for Render deployment"
   git push origin main
   ```

### 3. Deploy on Render

1. **Create Render Account**:
   - Go to [Render.com](https://render.com)
   - Sign up with GitHub

2. **Create New Web Service**:
   - Click "New" → "Web Service"
   - Connect your GitHub repository
   - Select your `flutter-trekify-project-backend` repository

3. **Configure Service Settings**:
   ```
   Name: trekify-backend
   Environment: Docker
   Region: Choose closest to your users
   Branch: main
   Dockerfile Path: ./TrekifyBackend/Dockerfile
   Docker Context: ./TrekifyBackend
   ```

4. **Set Environment Variables**:
   - In Render dashboard, go to "Environment"
   - Add these variables:
   
   ```
   ASPNETCORE_ENVIRONMENT = Production
   ASPNETCORE_URLS = http://0.0.0.0:$PORT
   MONGODB_URI = your_mongodb_atlas_connection_string
   JWT_SECRET = your_secure_jwt_secret_key_32_chars_minimum
   ```

5. **Deploy**:
   - Click "Create Web Service"
   - Render will automatically build and deploy your app

### 4. Alternative: Manual Docker Deployment

If the automatic deployment doesn't work, use these settings:

**Build Command**:
```bash
cd TrekifyBackend && dotnet restore && dotnet build --configuration Release && dotnet publish --configuration Release --output ./publish
```

**Start Command**:
```bash
cd TrekifyBackend/publish && dotnet TrekifyBackend.dll
```

**Environment**: Docker
**Dockerfile Path**: `./TrekifyBackend/Dockerfile`

### 5. Test Your Deployment

1. **Check the Logs**:
   - In Render dashboard, go to "Logs"
   - Verify the app starts without errors

2. **Test API Endpoints**:
   - Your app will be available at: `https://your-app-name.onrender.com`
   - Test base endpoint: `GET https://your-app-name.onrender.com/`
   - Test API docs: `https://your-app-name.onrender.com/swagger`

3. **Test API Endpoints**:
   ```
   GET https://your-app-name.onrender.com/api/data
   POST https://your-app-name.onrender.com/api/auth/register
   POST https://your-app-name.onrender.com/api/auth/login
   ```

### 6. Update Flutter App

Update your Flutter app's API base URL to point to your Render deployment:

```dart
// In your Flutter app
const String baseUrl = 'https://your-app-name.onrender.com';
```

### 7. Custom Domain (Optional)

1. In Render dashboard, go to "Settings" → "Custom Domains"
2. Add your domain name
3. Update DNS records as instructed

## Important Notes

### Free Tier Limitations
- Render free tier spins down after 15 minutes of inactivity
- First request after spin-down may take 30-60 seconds
- For production, consider upgrading to paid tier

### Environment Variables Security
- Never commit secrets to Git
- Use Render's environment variables for sensitive data
- Rotate JWT secrets regularly

### MongoDB Atlas Setup
- Whitelist Render's IP addresses (or use 0.0.0.0/0 for simplicity)
- Create a database user with appropriate permissions
- Use strong passwords

## Troubleshooting

### Common Issues:

1. **Build Fails**:
   - Check Dockerfile syntax
   - Verify all dependencies in .csproj
   - Check build logs in Render dashboard

2. **App Won't Start**:
   - Verify PORT environment variable usage
   - Check connection strings
   - Review application logs

3. **Database Connection Issues**:
   - Verify MongoDB Atlas connection string
   - Check network access settings in Atlas
   - Ensure database user has correct permissions

### Getting Help:
- Check Render documentation: https://render.com/docs
- Review deployment logs in Render dashboard
- Test locally with Docker first

## Cost Estimate
- **Render**: Free tier available, paid plans start at $7/month
- **MongoDB Atlas**: Free tier (512MB), paid plans start at $9/month
- **Total**: Free for development, ~$16/month for production
