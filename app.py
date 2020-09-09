from flask import Flask, request, session, redirect, render_template, jsonify, flash
from flask_session import Session

from werkzeug.security import check_password_hash, generate_password_hash

import sqlite3

conn = sqlite3.connect('classroom.db', check_same_thread=False)
cursor = conn.cursor()

app = Flask(__name__)

app.secret_key = 'dev'

app.config["SESSION_PERMANENT"] = False
app.config["SESSION_TYPE"] = "filesystem"
Session(app)

@app.route("/")
def index():
    return render_template("index.html")

@app.route("/register", methods=["POST", "GET"])
def register():
    if request.method == "GET":
        return render_template("register.html")
    else:
        username = request.form.get("username")
        if not username:
            return render_template("error.html", message="please provide username")

        username_check = cursor.execute("SELECT * FROM Players WHERE username=:username", {"username": username}).fetchone()
        if username_check:
            return render_template("error.html", message="username already exists")

        password = request.form.get("password")
        if not password:
            return render_template("error.html", message="please provide password")

        confirm_password = request.form.get("confirm_password")
        if not confirm_password:
            return render_template("error.html", message="please provide confirmation password")

        if not password == confirm_password:
            return render_template("error.html", message="passwords did not match")

        # Hash password to store in DB , method='pbkdf2:sha256', salt_length=8
        hashed_password = generate_password_hash(password)

        cursor.execute("INSERT INTO Players (username, password) VALUES (?, ?)",
                       (username, hashed_password))

        conn.commit()

        flash("Account created", 'info')

        return redirect("/login")


@app.route("/login", methods=["GET", "POST"])
def login():
    session.clear()

    if request.method == "POST":
        username = request.form.get("username")
        if not username:
            return render_template("error.html", message="please provide username")
        password = request.form.get("password")
        if not password:
            return render_template("error.html", message="please provide password")

        rows = cursor.execute("SELECT * FROM Players WHERE username= ?", [username])

        result = rows.fetchone()

        if result == None or not check_password_hash(result[2], password):
            return render_template("error.html", message="invalid username and/or password")

        # remember which user has logged in
        session["player_id"] = result[0]
        session["player_username"] = result[1]
        print(result[0])
        print(result[1])

        return redirect("/")
    else:
        return render_template("login.html")

@app.route("/logout")
def logout():
    session.clear()
    return redirect("/")

@app.route("/shop")
def shop():
    shopItems = cursor.execute("SELECT * FROM ShopItem").fetchall()
    if shopItems:
        print(shopItems)
    return render_template("shop.html", shopItems=shopItems)

@app.route("/avatarshop")
def avatarshop():
    return render_template("avatarshop.html")
        
if __name__ == '__main__':
    app.run()
