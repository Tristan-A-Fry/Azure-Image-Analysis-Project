import React, { useState } from 'react';
import axios from 'axios';

const ImageUpload = () => {
  const [selectedFile, setSelectedFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [analysisResult, setAnalysisResult] = useState(null);

  // This is for when I add the ability to store the image on the server side
  //const [uploadedImageUrl, setUploadedImageUrl] = useState(null);

  const handleFileChange = (event) => {
    setSelectedFile(event.target.files[0]);
  };

  const handleUpload = () => {
    if (!selectedFile) {
      alert('Please select a file.');
      return;
    }

    setLoading(true);
    setError(null);

    const apiManagementUrl  = "http://localhost:5006"; 
    const formData = new FormData();
    formData.append('file', selectedFile);
    //.post('http://localhost:5007/api/ImageAnalysis', formData)
    axios
      .post(`${apiManagementUrl}/api/ImageAnalysis`, formData)
      .then((response) => {
        setLoading(false);
        if (response.data && response.data.caption) {
          setAnalysisResult(response.data);
          
          //setUploadedImageUrl(response.data.imageUrl); // Set the uploaded image URL
          
        } else {
          setError('Invalid API response');
        }
      })
      .catch((error) => {
        setLoading(false);
        console.error('Error uploading file', error);
        setError('Error uploading file');
      });
  };

  return (
    <div>
      <h1>Image Upload</h1>
      <form>
        <input type="file" onChange={handleFileChange} />
        <button type="button" onClick={handleUpload} disabled={loading}>
          Upload
        </button>
      </form>
      {loading && <p>Uploading...</p>}
      {error && <p>{error}</p>}
      {analysisResult && (
        <div>
          <h2>Analysis Result:</h2>
          <p>Caption: {analysisResult.caption}</p>
          <p>Confidence: {analysisResult.confidence}</p>
          {/* {uploadedImageUrl && (
            <div>
              <h2>Uploaded Image:</h2>
              <img src={uploadedImageUrl} alt="Uploaded" />
            </div>
          )} */}
        </div>
      )}
    </div>
  );
};

export default ImageUpload;
