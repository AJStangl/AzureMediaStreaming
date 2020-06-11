import React, {Component} from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>Patterson Park Crime Wire</h1>
        <p>A simple way for the community to share videos of crimes in one place</p>
        <p>To help you get started, we have also set up a simple portal for:</p>
        <ul>
          <li><strong>Search for videos by zip code and address</strong>. For example, click <em>Search For Videos</em> then <em>Back</em> to return here.</li>
          <li><strong>Upload your own videos</strong>.For example, click <em>Upload video</em> then <em>Back</em> to return here.</li>
        </ul>
      </div>
    );
  }
}
