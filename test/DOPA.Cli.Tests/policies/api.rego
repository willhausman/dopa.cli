package api

default allow = false

allow {
    input.method == "GET"
    
    path := split(input.path, "/")
    path[0] == "public"
}

allow {
    data.users[input.user].roles.admin
}

allow {
    input.method = "GET"
    path := split(input.path, "/")
    path == ["users", employeeId, "salary"]
    data.users[employeeId].manager[_] == input.user
}

test_deny {
    not allow
    not allow with input as { "method": "POST" }
}

test_allow_public {
    allow with input as { "path": "public/document.pdf", "method": "GET" }
    
    allow with input as { "path": "public/images/profile.png", "method": "GET" }
    allow with input as { "path": "public/reports.xlsx", "method": "GET" }
}

test_allow_admin {
    allow
        with input as { "path": "private/document.pdf", "method": "POST", "user": "alice" }
        with data.users.alice.roles.admin as true
}

test_allow_salary {
    allow 
        with input as { "path": "users/alice/salary", "method": "GET", "user": "bob" }
        with data.users.alice.manager as ["bob"]
}
