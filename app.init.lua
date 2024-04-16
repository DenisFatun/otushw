#!/usr/bin/env tarantool

local log = require('log')
local uuid = require('uuid')

local function init()
    local posts_space = box.schema.space.create('posts', {
        if_not_exists = true
    })

    posts_space:create_index('id', {
        if_not_exists = true,
        unique = true,
        parts = { 1 }
    })

    posts_space:create_index('author', {
        if_not_exists = true,
        unique = false,
        parts = { 2 }
    })
end

box.cfg
{
    pid_file = nil,
    background = false,
    log_level = 5
}

box.once('init', init)

local function posts_list(self)
    local author = self:stash('id')
    local data = {}

    for _, tuple in box.space.posts.index.author:pairs{author} do
        table.insert(data, {
                id = tuple[1], 
                author = tuple[2], 
                created = tuple[3],
                text = tuple[4]                
        })
    end
    
    return self:render{ json = data }
end

local function posts_save(req)
    local data = req:json()

    local posts_space = box.space.posts

    posts_space:auto_increment{data.author, 
    data.created_at, data.post_text}

    local resp = req:render({text = 'Post created' })
    return resp
end

local httpd = require('http.server')
local server = httpd.new(nil, 8080, nil)

server:route({ path = '/posts/:id'  }, posts_list)
server:route({ path = '/posts', method = 'POST' }, posts_save)
server:start()