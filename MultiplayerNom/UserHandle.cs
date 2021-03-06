﻿using System;
using ByteNom;

namespace MultiplayerNom
{
    internal class UserHandle
    {
        private IRoomInternal _room;

        public UserHandle(Connection connection, IRoomInternal room, int userId)
        {
            this.UserId = userId;
            this.Connection = connection;
            if (!this.MoveToRoom(room))
                throw new OperationCanceledException("Could not join the room!");

            connection.MessageReceived += (o, message) =>
            {
                if (this._room == null) return;
                this._room.HandleMessage(this, message);
            };

            connection.Disconnected += (o, args) =>
            {
                if (this._room == null) return;
                this._room.RemoveUser(this);
            };
        }

        public Connection Connection { get; private set; }
        public int UserId { get; private set; }

        public bool MoveToRoom(IRoomInternal newRoom)
        {
            if (newRoom == null)
                throw new ArgumentNullException("newRoom");

            if (newRoom.AddUser(this))
            {
                IRoomInternal oldRoom = this._room;
                this._room = newRoom;
                if (oldRoom != null)
                    oldRoom.RemoveUser(this);

                return true;
            }
            return false;
        }
    }
}