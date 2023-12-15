# BlogApp Documentation

## Overview

BlogApp is an application for creating personal blogs, posts within these blogs, and allowing for comments from other users. It utilizes technologies such as WebAPI, Blazor Server, LINQ, JWT, SignalR, and others.

## Key Features

- **User Registration and Authentication:** Users can register, log in, and log out. JWT Authentication is used to ensure secure access.
- **CRUD Operations:** Complete Create, Read, Update, and Delete functionalities are implemented for blogs and posts. Creation can be made by authenticated users, while Edit/Delete operations are reserved for resource owners.
- **Tagging System:** Posts can be tagged for better categorization and searchability.
- **Search Functionality:** Users can search for specific tags and other users, enhancing the ability to discover relevant content.

## Architecture

- **Layered Architecture:** The application is structured into layers, including a database for data storage, a backend built on WebAPI for handling business logic, and a frontend developed using Blazor Server for user interactions.

### Chat Functionality

#### Overview

The chat feature in this application provides a real-time communication platform for users. It is designed to allow users to exchange messages instantaneously, enhancing interaction and engagement within the application. The chat interface is user-friendly and accessible, making it easy for users to start conversations, share information, and collaborate.

Key aspects of the chat functionality include:

- **Real-time Messaging:** Users can send and receive messages instantly without any noticeable delay.
- **Chat History:** The application saves chat history, so users can review previous conversations when they revisit the chat.
- **History Management:** Users have the ability to clear their chat history, providing control over their data and conversation records.
- **Multi-line Message Support:** The chat supports multi-line messages. Users can press 'Enter' for sending messages and 'Shift + Enter' to insert a new line within a message.
- **User Authentication:** Chat access is integrated with user authentication, ensuring that only authenticated users can participate in conversations.

#### Usage

Using the chat feature is straightforward:

1. **Access Chat:** Navigate to the chat section after logging into the application.
2. **Compose Message:** Type your message in the provided text input field.
3. **Send Message:** Press 'Enter' to send the message.
4. **Multi-line Messages:** To create a multi-line message, press 'Shift + Enter' to add a new line within your message.
5. **View Chat History:** Previous messages in the chat session are displayed in the chat window, allowing users to scroll through past conversations.
6. **Clear Chat History:** Click the 'Clear chat' button to permanently remove the current chat history from the session.

This chat functionality is part of our commitment to enhance user experience and foster effective communication within the application.n 
